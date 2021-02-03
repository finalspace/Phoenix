using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace C1.Feedbacks
{
    public class C1FeedbackAttributeUtils : MonoBehaviour
    {
    }

    public class C1FeedbackAttributeName : System.Attribute
    {
        public string Name;

        public C1FeedbackAttributeName(string name)
        {
            Name = name;
        }

        static public string GetFeedbackName(System.Type type)
        {
            var attribute = type.GetCustomAttributes(false).OfType<C1FeedbackAttributeName>().FirstOrDefault();
            return attribute != null ? attribute.Name : type.Name;
        }
    }
}
