using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using VisualProtobuf.Condition;

namespace VisualProtobuf
{
    /// <summary>
    /// Langueage code for l10n ,see https://en.wikipedia.org/wiki/Language_localisation.
    /// </summary>
    public enum LanguageCode
    {
        Default = 0,
        ZH_CN,
    }

    public static partial class ProtobufAnnotationExtension
    {
        private const string kAnnotation_Common_Name = "name_"; // @name_zh(l10n name,[tooltip])
        private const string kAnnotation_Message_Schema = "schema"; // @schema
        private const string kAnnotation_Field_Range = "range"; // @range(min,max,[percision]) 
        private const string kAnnotation_Field_Percent = "percent"; // @percent 
        private const string kAnnotation_Field_Condition = "condition"; // @condition(expression)

        #region IDescriptor
        internal static string ToAnnotation(this LanguageCode code)
        {
            return code switch
            {
                LanguageCode.ZH_CN => "zh",
                _ => null,
            };
        }

        public static string GetName(this IDescriptor descriptor, LanguageCode languageCode)
        {
            var code = languageCode.ToAnnotation();
            if ((!string.IsNullOrEmpty(code)) && descriptor.TryGetAnnotation(kAnnotation_Common_Name + code, out var annotation))
            {
                if (annotation.TryGetParameter(0, out var name))
                {
                    return name;
                }
            }
            return descriptor.Name;
        }

        public static string GetTooltip(this IDescriptor descriptor, LanguageCode languageCode)
        {
            var code = languageCode.ToAnnotation();
            if ((!string.IsNullOrEmpty(code)) && descriptor.TryGetAnnotation("name_" + code, out var annotation))
            {
                if (annotation.TryGetParameter(1, out var tooltip))
                {
                    return tooltip;
                }
            }
            return null;
        }

        #endregion

        #region MessageDescriptor 


        public static bool IsSchema(this MessageDescriptor msgDesc)
        {
            return msgDesc.HasAnnotation(kAnnotation_Message_Schema);
        }
        #endregion

        #region EnumDescriptor 
        #endregion

        #region EnumValue Descriptor 
        #endregion

        #region FieldDescriptor 



        public static void TryGetValueRange<T>(this FieldDescriptor fieldDescriptor, out T min, out T max, out bool slider) where T : struct, System.IComparable<T>
        {
            Annotation.GetNumberMinMaxValue<T>(out var typeMin, out var typeMax);
            if (fieldDescriptor.TryGetAnnotation(kAnnotation_Field_Range, out var annotation))
            {
                min = annotation.GetParameterAsNumber(0, typeMin);
                if (min.CompareTo(typeMin) < 0) min = typeMin;
                max = annotation.GetParameterAsNumber(1, typeMax);
                if (max.CompareTo(typeMax) > 0) max = typeMax;
                slider = annotation.GetParameterAsBool(2, false);
                return;
            }
            min = typeMin;
            max = typeMax;
            slider = false;
        }

        public static bool IsPercent(this FieldDescriptor fieldDescriptor)
        {
            if (fieldDescriptor.FieldType != FieldType.Float) return false;
            return fieldDescriptor.HasAnnotation(kAnnotation_Field_Percent);
        }

        public static bool TryGetPercentPrecision(this FieldDescriptor fieldDescriptor, out int precision)
        {
            if (fieldDescriptor.FieldType == FieldType.Float && fieldDescriptor.TryGetAnnotation(kAnnotation_Field_Percent, out var annotation))
            {
                if (annotation.TryGetParameterAsNumber(0, out precision))
                {
                    return true;
                }
            }
            precision = default;
            return false;
        }

        internal class ConditionAnnotation : Annotation
        {
            internal LogicExpression conditionExpression;

            internal ConditionAnnotation(string exp) : base(null, null)
            {
                conditionExpression = Parser.ParseLogic(exp);
            }
        }

        public static bool TryGetConditionExpression(this FieldDescriptor fieldDescriptor, out LogicExpression expression)
        {
            if (!fieldDescriptor.TryGetAnnotation(kAnnotation_Field_Condition, out var annotation))
            {
                expression = null;
                return false;
            }
            if (annotation is ConditionAnnotation conditionAnnotation)
            {
                expression = conditionAnnotation.conditionExpression;
                return true;
            }
            if (!annotation.TryGetParameter(0, out var expString))
            {
                expression = null;
                return false;
            }
            try
            {
                conditionAnnotation = new ConditionAnnotation(expString);
                expression = conditionAnnotation.conditionExpression;
                fieldDescriptor.SetAnnotation(kAnnotation_Field_Condition, conditionAnnotation);
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Parse condition {expString} failed, exception: {e.Message}");
                expression = null;
                return false;
            }
        }

        public static bool GetConditionResult(this FieldDescriptor fieldDescriptor, IMessage message)
        {
            if (!fieldDescriptor.TryGetConditionExpression(out var logicExpression))
            {
                // default true
                return true;
            }
            try
            {
                foreach (var variable in logicExpression.Variables)
                {
                    var fullName = message.Descriptor.FullName + "." + variable.Name;
                    var varFieldDesc = fieldDescriptor.File.FindFieldByFullName(fullName);
                    if (varFieldDesc == null)
                    {
                        Debug.LogError($"Condition expression variable {fullName} not found!");
                        return true;
                    }
                    if (varFieldDesc.FieldType == FieldType.Bool)
                    {
                        variable.Set(varFieldDesc.GetValue<bool>(message));
                    }
                    else if (varFieldDesc.IsNumeric())
                    {
                        var value = varFieldDesc.Accessor.GetValue(message);
                        var doubleVal = System.Convert.ToDouble(value);
                        variable.Set(doubleVal);
                    }
                    else
                    {
                        Debug.LogError($"Condition expression variable {fullName} is invalid type {varFieldDesc.FieldType}");
                        return true;
                    }
                }
                return logicExpression.GetResult();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Check condition exception: {e.Message}");
                return true;
            }
        }
        #endregion
    }
}