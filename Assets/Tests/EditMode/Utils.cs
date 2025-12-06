using System.Reflection;

namespace Tests.EditMode
{
    public static class Utils
    {
        public static void SetPrivateField<T>(object target, string fieldName, T value)
        {
            var field = target.GetType()
                .GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

            field.SetValue(target, value);
        }
    }
}