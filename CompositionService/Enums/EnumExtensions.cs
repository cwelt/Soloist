using System;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CW.Soloist.CompositionService.Enums
{
    /// <summary>
    /// Enumeration static class for providing general extensions for enum types,
    /// such as friendly descriptions for the individual enum values, 
    /// which could be displayed in user interfaces and string representations.
    /// </summary>
    public static class EnumExtensions
    {
        #region GetDescription
        /// <summary>
        /// Returns a friendly string representation for the given enum value,
        /// which is defined for it in a custom <see cref="DescriptionAttribute"/>
        /// annotation.
        /// </summary>
        /// <param name="value"> The enumeration underlying value. </param>
        /// <returns> The friendly string description for the given value. </returns>
        public static string GetDescription(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo fieldInfo = type.GetField(name);
                if (fieldInfo != null)
                {
                    DescriptionAttribute descriptionAttribiute =  Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (descriptionAttribiute != null)
                    {
                        return descriptionAttribiute.Description;
                    }
                }
            }
            return null;
        }
        #endregion

        #region GetDisplayName
        /// <summary>
        /// Returns a friendly string representation for the given enum value,
        /// which is defined for it in a custom <see cref="DisplayAttribute"/>
        /// annotation. This extension is primarily used for supporting the display 
        /// of the friendly descriptions in ASP.NET MVC applications. 
        /// </summary>
        /// <param name="enumValue"> The enumeration underlying value. </param>
        /// <returns> The friendly string description for the given value. </returns>
        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<DisplayAttribute>()
                            .GetName();
        }
        #endregion
    }
}
