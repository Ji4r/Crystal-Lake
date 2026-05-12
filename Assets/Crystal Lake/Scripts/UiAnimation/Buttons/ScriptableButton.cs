using DG.Tweening;
using UnityEngine;

namespace MyProj
{
    [CreateAssetMenu(fileName = "ScriptableButtonAnims", menuName = "SO/Button/ButtonPreset")]
    public class ScriptableButton : ScriptableObject
    {
        [Header("При наведении на кнопку")]
        [SerializeField] private float addSizeOnHover = 0.15f;
        [SerializeField] private float durationAnimsOnHover = 0.2f;
        [SerializeField] private Ease easeScheduleHover = Ease.OutCubic;

        [Header("При нажатии на кнопку")]
        [SerializeField] private float addSizeOnClick = 0.25f;
        [SerializeField] private float durationAnimsOnClick = 0.2f;
        [SerializeField] private Ease easeScheduleClick = Ease.OutCubic;

        [Header("Движение текста при наведении")]
        [SerializeField] private float addPositionByX;
        [SerializeField] private float durationOffsetsPositionByX;
        [SerializeField] private Ease easeMoveHover = Ease.OutCubic;

        public float AddSizeOnHover => addSizeOnHover;
        public float DurationAnimsOnHover => durationAnimsOnHover;
        public Ease EaseScheduleHover => easeScheduleHover;

        public float AddSizeOnClick => addSizeOnClick;
        public float DurationAnimsOnClick => durationAnimsOnClick;
        public Ease EaseScheduleClick => easeScheduleClick;

        public float AddPositionByX => addPositionByX;
        public float DurationOffsetsPositionByX => durationOffsetsPositionByX;
        public Ease EaseMoveHover => easeMoveHover;
    }
}
