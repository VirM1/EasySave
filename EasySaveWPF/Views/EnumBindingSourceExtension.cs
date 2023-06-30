using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Markup;

namespace EasySaveWPF.Views
{
    public class EnumBindingSourceExtension : MarkupExtension
    {
        public Type EnumType { get; private set; }

        public EnumBindingSourceExtension(Type enumType)
        {
            if(enumType is null || !enumType.IsEnum)
            {
                throw new Exception("Enum provided not valid enum");
            }
            EnumType = enumType;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Enum.GetValues(EnumType);
        }
    }
}
