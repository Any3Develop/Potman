namespace Potman.Game.Stats
{
    public class ModifiableInt
    {
        public int Total => Value + Modifier;
        public int Value { get; private set; }
        public int Modifier { get; private set; }

        public ModifiableInt() => (Value, Modifier) = (0, 0);
        public ModifiableInt(int value) => (Value, Modifier) = (value, 0);
        public ModifiableInt(int value, int modifier) : this(value) => Modifier = modifier;
        
        public ModifiableInt Override(int value)
        {
            Value = value;
            return this;
        }

        public ModifiableInt Set(int value)
        {
            Modifier = value;
            return this;
        }

        public ModifiableInt Add(int value)
        {
            Modifier += value;
            return this;
        }

        public ModifiableInt Subtract(int value)
        {
            Modifier -= value;
            return this;
        }

        public static implicit operator int(ModifiableInt v) => v.Total;
    }
}