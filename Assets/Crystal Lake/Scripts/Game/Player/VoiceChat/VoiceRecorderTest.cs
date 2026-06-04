using Mirror;
using Steamworks;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public class VoiceRecorderTest : NetworkBehaviour
{
    private AudioSource audioSource;
    private AudioClip streamingClip;

    private readonly ConcurrentQueue<float> voiceQueue = new();
    private readonly List<byte> sendBuffer = new();

    private byte[] decompressedBuffer = new byte[65535];
    private byte[] voiceBuffer = new byte[8192];

    private float nextSendTime;
    private int emptyReads;
    private float flushTimer;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        streamingClip = AudioClip.Create(
            "VoiceStream",
            22050 * 5,
            1,
            22050,
            true,
            OnAudioRead);

        audioSource.clip = streamingClip;
        audioSource.loop = true;
        audioSource.Play();
    }

    public override void OnStartLocalPlayer()
    {
        SteamUser.StartVoiceRecording();
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        if (Time.time < nextSendTime)
            return;

        nextSendTime = Time.time + 0.02f; // 50 раз в секунду

        uint availableBytes;

        if (SteamUser.GetAvailableVoice(out availableBytes) != EVoiceResult.k_EVoiceResultOK)
            return;

        if (availableBytes == 0)
            return;

        uint bytesWritten;

        Debug.Log($"Available: {availableBytes}");
        if (SteamUser.GetVoice(
            true,
            voiceBuffer,
            (uint)voiceBuffer.Length,
            out bytesWritten) != EVoiceResult.k_EVoiceResultOK)
            return;

        for (int i = 0; i < bytesWritten; i++)
        {
            sendBuffer.Add(voiceBuffer[i]);
        }

        if (sendBuffer.Count >= 400)
        {
            CmdSendVoice(sendBuffer.ToArray());
            sendBuffer.Clear();
        }

        flushTimer += Time.deltaTime;

        if (sendBuffer.Count >= 400 || flushTimer >= 0.1f)
        {
            if (sendBuffer.Count > 0)
            {
                CmdSendVoice(sendBuffer.ToArray());
                sendBuffer.Clear();
            }

            flushTimer = 0f;
        }
    }

    [Command(channel = Channels.Unreliable)]
    private void CmdSendVoice(byte[] data)
    {
        RpcReceiveVoice(data);
    }

    [ClientRpc(channel = Channels.Unreliable)]
    private void RpcReceiveVoice(byte[] data)
    {
        if (isLocalPlayer)
            return;

        uint bytesWritten;

        EVoiceResult result = SteamUser.DecompressVoice(
            data,
            (uint)data.Length,
            decompressedBuffer,
            (uint)decompressedBuffer.Length,
            out bytesWritten,
            22050
        );
        Debug.Log($"Decompressed: {bytesWritten}");

        if (result != EVoiceResult.k_EVoiceResultOK)
            return;

        PlayVoice(decompressedBuffer, (int)bytesWritten);
    }

    private void OnDestroy()
    {
        if (isLocalPlayer)
            SteamUser.StopVoiceRecording();
    }

    private void PlayVoice(byte[] pcmData, int length)
    {
        int sampleCount = length / 2;

        for (int i = 0; i < sampleCount; i++)
        {
            short sample = System.BitConverter.ToInt16(pcmData, i * 2);
            voiceQueue.Enqueue(sample / 32768f);
        }
    }

    private void OnAudioRead(float[] data)
    {
        emptyReads = 0;

        bool hadData = false;

        for (int i = 0; i < data.Length; i++)
        {
            if (voiceQueue.TryDequeue(out float sample))
            {
                data[i] = sample;
                hadData = true;
            }
            else
            {
                data[i] = 0f;
            }
        }

        if (!hadData)
            emptyReads++;
    }
}