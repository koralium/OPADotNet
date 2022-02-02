using OPADotNet.Reasons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPADotNet.AspNetCore.Reasons
{
    internal class DefaultFormatterVisitor : ReasonVisitor<string, int>
    {
        private static readonly string AndText    = $"All must be true:{Environment.NewLine}";
        private static readonly string OrText     = $"One must be true:{Environment.NewLine}";
        public override string VisitReasonMessage(ReasonMessage reasonMessage, int state)
        {
            int extraPadding = 0;
            var formattedMessage = reasonMessage.Message;
            if (reasonMessage.Message != null)
            {
                extraPadding = 2;
                formattedMessage = String.Format(reasonMessage.Message, reasonMessage.Variables.ToArray());
            }
            if (reasonMessage.AndConditions.Count > 1)
            {
                var text = Visit(reasonMessage.AndConditions, state + extraPadding).Select(x =>
                {
                    return x;
                }).Where(x => !string.IsNullOrEmpty(x));

                var conditionText = AndText + String.Join(Environment.NewLine, text.Select(x => $"* {x}").Select(x => x.PadLeft(x.Length + state)));
                if (reasonMessage.Message == null)
                {
                    return conditionText;
                }
                else
                {
                    return formattedMessage + Environment.NewLine + conditionText;
                }
            }
            // Single and condition
            else if (reasonMessage.AndConditions.Count == 1)
            {
                var text = Visit(reasonMessage.AndConditions.First(), state + extraPadding);

                if (reasonMessage.Message == null)
                {
                    return text;
                }
                else
                {
                    var t = "* " + text;
                    return formattedMessage + Environment.NewLine + t.PadLeft(t.Length + state);
                }
            }
            // No children
            else
            {
                if (reasonMessage.Message == null)
                {
                    return string.Empty;
                }
                else
                {
                    return formattedMessage;
                }
            }
        }

        public override string VisitReasonAndCondition(ReasonAndCondition reasonAndCondition, int state)
        {
            if (reasonAndCondition.OrConditions.Count > 1)
            {
                var text = Visit(reasonAndCondition.OrConditions, state + 2).Select(x => x).Where(x => !string.IsNullOrEmpty(x));
                var conditionText = OrText + String.Join(Environment.NewLine, text.Select(x => $"- {x}").Select(x => x.PadLeft(x.Length + state)));
                return conditionText;
            }
            else if (reasonAndCondition.OrConditions.Count == 1)
            {
                var text = Visit(reasonAndCondition.OrConditions.First(), state);
                return text;
            }
            else
            {
                throw new NotSupportedException("Can not have an and condition without any or conditions for reasons, at least one condition must exist");
            }
        }
    }
}
