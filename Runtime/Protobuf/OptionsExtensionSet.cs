using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;
using log4net.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VisualProtobuf.Extensions
{
    public class OptionsExtensionSet : IOptionsExtension
    {
        public void TryResolveOptions<T>(IExtendableMessage<T> options, RepeatedField<UninterpretedOption> uninterpretedOptions) where T : IExtendableMessage<T>
        {
            foreach (var uninterpretedOption in uninterpretedOptions)
            {
                var name = GetExtendeeName(uninterpretedOption);
                if (string.IsNullOrEmpty(name)) continue;
                if (options is FileOptions fileOptions)
                {
                    //todo
                }
                else if (options is MessageOptions messageOptions)
                {
                    //todo
                }
                else if (options is FieldOptions fieldOptions)
                {
                    ResolveFieldOption(fieldOptions, name, uninterpretedOption);
                }
            }
        }

        string GetExtendeeName(UninterpretedOption option)
        {
            foreach (var name in option.Name)
            {
                if (name.IsExtension)
                {
                    return name.NamePart_;
                }
            }
            return null;
        }

        void ResolveFieldOption(IExtendableMessage<FieldOptions> options, string extendee, UninterpretedOption uninterpretedOption)
        {
            switch (extendee)
            {
                case "field_option_string":
                    options.SetExtension(OptionsExtensions.FieldOptionString, uninterpretedOption.AggregateValue);
                    break;
                default:
                    break;
            }
        }
    }
}
