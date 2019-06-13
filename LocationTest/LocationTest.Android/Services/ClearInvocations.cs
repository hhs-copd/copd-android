using System;
using System.Reflection;

namespace LocationTest.Droid.Services
{
    internal static class ClearInvocations
    {
        public static void ClearEventInvocations(this object obj, string eventName)
        {
            FieldInfo fi = GetEventField(obj.GetType(), eventName);
            if (fi == null)
            {
                return;
            }

            fi.SetValue(obj, null);
        }

        public static FieldInfo GetEventField(this Type type, string eventName)
        {
            FieldInfo field = null;
            while (type != null)
            {
                /* Find events defined as field */
                field = type.GetField(eventName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (field != null && (field.FieldType == typeof(MulticastDelegate) || field.FieldType.IsSubclassOf(typeof(MulticastDelegate))))
                {
                    break;
                }

                /* Find events defined as property { add; remove; } */
                field = type.GetField("EVENT_" + eventName.ToUpper(), BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
                if (field != null)
                {
                    break;
                }

                type = type.BaseType;
            }
            return field;
        }
    }
}