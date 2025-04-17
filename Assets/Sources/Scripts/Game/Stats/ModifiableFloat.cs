namespace Potman.Game.Stats
{
    public class ModifiableFloat
    {
        public float Total => Value + Modifier;
        public float Value { get; private set; }
        public float Modifier { get; private set; }

        public ModifiableFloat() => (Value, Modifier) = (0, 0);
        public ModifiableFloat(float value) => (Value, Modifier) = (value, 0);
        public ModifiableFloat(float value, float modifier) => (Value, Modifier) = (value, modifier);
        
        public ModifiableFloat Override(float value)
        {
            Value = value;
            return this;
        }

        public ModifiableFloat Set(float value)
        {
            Modifier = value;
            return this;
        }

        public ModifiableFloat Add(float value)
        {
            Modifier += value;
            return this;
        }

        public ModifiableFloat Subtract(float value)
        {
            Modifier -= value;
            return this;
        }

        public static implicit operator float(ModifiableFloat v) => v.Total;
    }
}