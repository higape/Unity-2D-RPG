using System;

namespace Static
{
    /// <summary>
    /// 元素抗性组，包含每种元素的抗性
    /// </summary>
    [Serializable]
    public class ElementGroup
    {
        public int Normal; //通常
        public int Corrosion; //腐蚀
        public int Fire; //火焰
        public int Ice; //冷冻
        public int Electricity; //电气
        public int Wave; //音波
        public int Ray; //光线
        public int Gas; //气体

        public int GetValue(ElementType elementType) =>
            elementType switch
            {
                ElementType.Normal => Normal,
                ElementType.Corrosion => Corrosion,
                ElementType.Fire => Fire,
                ElementType.Ice => Ice,
                ElementType.Electricity => Electricity,
                ElementType.Wave => Wave,
                ElementType.Ray => Ray,
                ElementType.Gas => Gas,
                _ => 0,
            };

        public int GetValue(ElementType elementType, float rate) =>
            100 - (int)((100 - GetValue(elementType)) * rate);

        public float GetRate(ElementType elementType) => (100 - GetValue(elementType)) / 100f;

        public float GetRate(ElementType elementType, float rate) => GetRate(elementType) * rate;
    }
}
