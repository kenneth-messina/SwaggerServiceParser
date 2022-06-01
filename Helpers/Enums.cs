using System;
using System.Linq;

namespace SwaggerServiceParser.Helpers
{
    public static class EnumHelper
    {
        public static T GetAttribute<T>(Enum enumValue) where T : Attribute
        {
            T attribute;
            var memberInfo = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault();

            if (memberInfo != null)
            {
                attribute = (T)memberInfo.GetCustomAttributes(typeof(T), false).FirstOrDefault();
                return attribute;
            }
            return null;
        }
    }

    public class EndPointAttribute : Attribute
    {
        public string Description { get; private set; }
        public EndPointAttribute(string description)
        {
            this.Description = description;
        }
    }

    public enum EndPointType
    {
        [EndPointAttribute("Get")]
        Get,
        [EndPointAttribute("Post")]
        Post,
        [EndPointAttribute("Put")]
        Put,
        [EndPointAttribute("Delete")]
        Delete
    }
}