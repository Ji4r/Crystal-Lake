using Steamworks;
using UnityEngine;

namespace MyProj
{
    public static class SteamExtenshionalTool
    {
        public static Texture2D GetSteamAvatar(CSteamID steamId)
        {
            int imageId =
                SteamFriends.GetLargeFriendAvatar(steamId);

            if (imageId == -1)
                return null;

            SteamUtils.GetImageSize(
                imageId,
                out uint width,
                out uint height
            );

            byte[] image =
                new byte[width * height * 4];

            SteamUtils.GetImageRGBA(
                imageId,
                image,
                image.Length
            );

            Texture2D texture = new Texture2D(
                (int)width,
                (int)height,
                TextureFormat.RGBA32,
                false
            );

            texture.LoadRawTextureData(image);
            texture.Apply();

            FlipTexture(texture);

            return texture;
        }

        private static void FlipTexture(Texture2D texture)
        {
            Color[] pixels = texture.GetPixels();
            Color[] flipped = new Color[pixels.Length];

            int width = texture.width;
            int height = texture.height;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    flipped[x + y * width] =
                        pixels[x + (height - y - 1) * width];
                }
            }

            texture.SetPixels(flipped);
            texture.Apply();
        }
    }
}