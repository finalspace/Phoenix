using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace C1.Feedbacks
{
    [CustomEditor(typeof(C1Feedbacks))]
    public class C1FeedbacksEditor : Editor
    {
        public class FeedbackTypePair
        {
            public System.Type FeedbackType;
            public string FeedbackName;
        }

        protected SerializedProperty c1feedbacks;
        protected Dictionary<C1Feedback, Editor> editors;
        protected List<FeedbackTypePair> typesAndNames = new List<FeedbackTypePair>();
        protected string[] typeDisplays;
        protected int draggedStartID = -1;
        protected int draggedEndID = -1;

        public static Texture2D paneOptionsIcon;
        public Color reorderdark = new Color(0.5f, 0.5f, 0.5f, 0.2f);

        // Start is called before the first frame update
        void OnEnable()
        {
            paneOptionsIcon = (Texture2D)EditorGUIUtility.Load("Builtin Skins/DarkSkin/Images/pane options.png");
            c1feedbacks = serializedObject.FindProperty("feedbacks");

            // Repair routine to catch feedbacks that may have escaped due to Unity's serialization issues
            RepairRoutine();

            editors = new Dictionary<C1Feedback, Editor>();
            for (int i = 0; i < c1feedbacks.arraySize; i++)
                AddEditor(c1feedbacks.GetArrayElementAtIndex(i).objectReferenceValue as C1Feedback);

            // Retrieve available feedbacks
            List<System.Type> feedbackTypes = (from domainAssembly in System.AppDomain.CurrentDomain.GetAssemblies()
                                       from assemblyType in domainAssembly.GetTypes()
                                       where assemblyType.IsSubclassOf(typeof(C1Feedback))
                                       select assemblyType).ToList();

            // Create display list from types
            List<string> typeNames = new List<string>();
            for (int i = 0; i < feedbackTypes.Count; i++)
            {
                FeedbackTypePair newType = new FeedbackTypePair();
                newType.FeedbackType = feedbackTypes[i];
                newType.FeedbackName = C1FeedbackAttributeName.GetFeedbackName(feedbackTypes[i]);
                typesAndNames.Add(newType);
            }

            typesAndNames = typesAndNames.OrderBy(t => t.FeedbackName).ToList();

            typeNames.Add("Add new feedback...");
            for (int i = 0; i < typesAndNames.Count; i++)
            {
                typeNames.Add(typesAndNames[i].FeedbackName);
            }

            typeDisplays = typeNames.ToArray();
        }

        /// <summary>
        /// Calls the repair routine if needed
        /// </summary>
        protected virtual void RepairRoutine()
        {
            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            var e = Event.current;

            // Update object
            serializedObject.Update();
            serializedObject.Update();

            Undo.RecordObject(target, "Modified Feedback Manager");

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Feedbacks", EditorStyles.boldLabel);

            for (int i = 0; i < c1feedbacks.arraySize; i++)
            {
                SerializedProperty property = c1feedbacks.GetArrayElementAtIndex(i);

                // Failsafe but should not happen
                if (property.objectReferenceValue == null)
                {
                    continue;
                }

                // Retrieve feedback
                C1Feedback feedback = property.objectReferenceValue as C1Feedback;
                Undo.RecordObject(feedback, "Modified Feedback");

                int id = i;
                bool isExpanded = property.isExpanded;
                string label = feedback.Label;

                Rect headerRect = DrawHeader(
                        ref isExpanded,
                        ref feedback.Active,
                        label,
                        feedback.FeedbackColor,
                        (GenericMenu menu) =>
                        {
                            menu.AddItem(new GUIContent("Remove"), false, () => RemoveFeedback(id));
                        });

                // Check if we start dragging this feedback
                switch (e.type)
                {
                    case EventType.MouseDown:
                        if (headerRect.Contains(e.mousePosition))
                        {
                            draggedStartID = i;
                            e.Use();
                        }
                        break;
                    default:
                        break;
                }

                // Draw blue rect if feedback is being dragged
                if (draggedStartID == i && headerRect != Rect.zero)
                {
                    Color color = new Color(0, 1, 1, 0.2f);
                    EditorGUI.DrawRect(headerRect, color);
                }

                // If hovering at the top of the feedback while dragging one, check where the feedback should be dropped : top or bottom
                if (headerRect.Contains(e.mousePosition))
                {
                    if (draggedStartID >= 0)
                    {
                        draggedEndID = i;

                        Rect headerSplit = headerRect;
                        headerSplit.height *= 0.5f;
                        headerSplit.y += headerSplit.height;
                        if (headerSplit.Contains(e.mousePosition))
                            draggedEndID = i + 1;
                    }
                }

                property.isExpanded = isExpanded;
                // If expanded, draw feedback editor
                if (property.isExpanded)
                {
                    //string helpText = FeedbackHelpAttribute.GetFeedbackHelpText(feedback.GetType());
                    string helpText = C1FeedbackAttributeName.GetFeedbackName(feedback.GetType());
                    if (!string.IsNullOrEmpty(helpText))
                    {
                        GUIStyle style = new GUIStyle(EditorStyles.helpBox);
                        style.richText = true;
                        float newHeight = style.CalcHeight(new GUIContent(helpText), EditorGUIUtility.currentViewWidth);
                        EditorGUILayout.LabelField(helpText, style);
                    }

                    EditorGUILayout.Space();

                    //?
                    //if (!editors.ContainsKey(feedback))
                    //    AddEditor(feedback);

                    Editor editor = editors[feedback];
                    CreateCachedEditor(feedback, feedback.GetType(), ref editor);
                    editor.OnInspectorGUI();

                    EditorGUI.BeginDisabledGroup(!Application.isPlaying);
                    EditorGUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("Play", EditorStyles.miniButtonMid))
                        {
                            PlayFeedback(id);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUI.EndDisabledGroup();

                    EditorGUILayout.Space();
                }
            }

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space();

            //////////////////////////////////////////////////////
            ///////////               Group             //////////
            EditorGUILayout.BeginHorizontal();
            {
                // Feedback list
                int newItem = EditorGUILayout.Popup(0, typeDisplays) - 1;
                if (newItem >= 0)
                {
                    AddFeedback(typesAndNames[newItem].FeedbackType);
                }
            }
            EditorGUILayout.EndHorizontal();
            ///////////               Group             //////////
            //////////////////////////////////////////////////////

            // Reorder
            if (draggedStartID >= 0 && draggedEndID >= 0)
            {
                if (draggedEndID != draggedStartID)
                {
                    if (draggedEndID > draggedStartID)
                        draggedEndID--;
                    c1feedbacks.MoveArrayElement(draggedStartID, draggedEndID);
                    draggedStartID = draggedEndID;
                }
            }

            if (draggedStartID >= 0 || draggedEndID >= 0)
            {
                switch (e.type)
                {
                    case EventType.MouseUp:
                        draggedStartID = -1;
                        draggedEndID = -1;
                        e.Use();
                        break;
                    default:
                        break;
                }
            }

            // Clean up
            bool wasRemoved = false;
            for (int i = c1feedbacks.arraySize - 1; i >= 0; i--)
            {
                if (c1feedbacks.GetArrayElementAtIndex(i).objectReferenceValue == null)
                {
                    wasRemoved = true;
                    c1feedbacks.DeleteArrayElementAtIndex(i);
                }
            }

            if (wasRemoved)
            {
                GameObject gameObject = (target as C1Feedbacks).gameObject;
                foreach (var c in gameObject.GetComponents<Component>())
                {
                    //c.hideFlags = HideFlags.None;
                }
            }

            // Apply changes
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// We need to repaint constantly if dragging a feedback around
        /// </summary>
        public override bool RequiresConstantRepaint()
        {
            return true;
        }


        Rect DrawHeader(ref bool expanded, ref bool activeField, string title, Color feedbackColor, System.Action<GenericMenu> fillGenericMenu)
        {
            var e = Event.current;

            // Initialize Rects
            var backgroundRect = GUILayoutUtility.GetRect(1f, 17f);

            var progressRect = GUILayoutUtility.GetRect(1f, 2f);

            var offset = 4f;

            var reorderRect = backgroundRect;
            reorderRect.xMin -= 8f;
            reorderRect.y += 5f;
            reorderRect.width = 9f;
            reorderRect.height = 9f;

            var labelRect = backgroundRect;
            labelRect.xMin += 32f + offset;
            labelRect.xMax -= 20f;

            var foldoutRect = backgroundRect;
            foldoutRect.y += 1f;
            foldoutRect.xMin += offset;
            foldoutRect.width = 13f;
            foldoutRect.height = 13f;

            var toggleRect = backgroundRect;
            toggleRect.x += 16f;
            toggleRect.xMin += offset;
            toggleRect.y += 2f;
            toggleRect.width = 13f;
            toggleRect.height = 13f;

            var menuIcon = paneOptionsIcon;
            var menuRect = new Rect(labelRect.xMax + 4f, labelRect.y + 4f, menuIcon.width, menuIcon.height);

            //_timingStyle.normal.textColor = EditorGUIUtility.isProSkin ? _timingDark : _timingLight;
            //_timingStyle.alignment = TextAnchor.MiddleRight;

            var colorRect = new Rect(labelRect.xMin, labelRect.yMin, 5f, 17f);
            colorRect.xMin = 0f;
            colorRect.xMax = 5f;
            EditorGUI.DrawRect(colorRect, feedbackColor);

            // Background rect should be full-width
            backgroundRect.xMin = 0f;
            backgroundRect.width += 4f;

            progressRect.xMin = 0f;
            progressRect.width += 4f;

            Color headerBackgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.2f);
            EditorGUI.DrawRect(backgroundRect, headerBackgroundColor);

            // Foldout
            expanded = GUI.Toggle(foldoutRect, expanded, GUIContent.none, EditorStyles.foldout);

            // Title
            using (new EditorGUI.DisabledScope(!activeField))
            {
                EditorGUI.LabelField(labelRect, title, EditorStyles.boldLabel);
            }

            /*
            float timingRectWidth = 150f;
            float totalTime = 0f;
            string timingInfo = "[ " + totalTime.ToString("F2") + "s ]";
            var timingRect = new Rect(labelRect.xMax - timingRectWidth, labelRect.yMin, timingRectWidth, 17f);
            timingRect.xMin = labelRect.xMax - timingRectWidth;
            timingRect.xMax = labelRect.xMax;
            EditorGUI.LabelField(timingRect, timingInfo);
            */

            EditorGUI.DrawRect(progressRect, headerBackgroundColor);

            GUIStyle smallTickbox = new GUIStyle("ShurikenToggle");
            // Active checkbox
            activeField = GUI.Toggle(toggleRect, activeField, GUIContent.none, smallTickbox);

            // Dropdown menu icon
            GUI.DrawTexture(menuRect, menuIcon);

            for (int i = 0; i < 3; i++)
            {
                Rect r = reorderRect;
                r.height = 1;
                r.y = reorderRect.y + reorderRect.height * (i / 3.0f);
                EditorGUI.DrawRect(r, reorderdark);
            }


            // Handle events

            if (e.type == EventType.MouseDown)
            {
                if (menuRect.Contains(e.mousePosition))
                {
                    var menu = new GenericMenu();
                    fillGenericMenu(menu);
                    menu.DropDown(new Rect(new Vector2(menuRect.x, menuRect.yMax), Vector2.zero));
                    e.Use();
                }
            }

            if (e.type == EventType.MouseDown && labelRect.Contains(e.mousePosition) && e.button == 0)
            {
                e.Use();
            }

            return backgroundRect;
        }


        /// <summary>
        /// Resets the selected feedback
        /// </summary>
        /// <param name="id"></param>
        protected virtual C1Feedback AddFeedback(System.Type type)
        {
            GameObject gameObject = (target as C1Feedbacks).gameObject;

            C1Feedback newFeedback = Undo.AddComponent(gameObject, type) as C1Feedback;
            newFeedback.Label = C1FeedbackAttributeName.GetFeedbackName(type);

            AddEditor(newFeedback);

            c1feedbacks.arraySize++;
            c1feedbacks.GetArrayElementAtIndex(c1feedbacks.arraySize - 1).objectReferenceValue = newFeedback;

            return newFeedback;
        }

        /// <summary>
        /// Remove the selected feedback
        /// </summary>
        protected virtual void RemoveFeedback(int id)
        {
            SerializedProperty property = c1feedbacks.GetArrayElementAtIndex(id);
            C1Feedback feedback = property.objectReferenceValue as C1Feedback;

            (target as C1Feedbacks).feedbacks.Remove(feedback);

            editors.Remove(feedback);
            Undo.DestroyObjectImmediate(feedback);
        }



        //
        // Editors management
        //

        /// <summary>
        /// Create the editor for a feedback
        /// </summary>
        protected virtual void AddEditor(C1Feedback feedback)
        {
            if (feedback == null)
                return;

            if (!editors.ContainsKey(feedback))
            {
                Editor editor = null;
                CreateCachedEditor(feedback, null, ref editor);

                editors.Add(feedback, editor as Editor);
            }
        }

        /// <summary>
        /// Destroy the editor for a feedback
        /// </summary>
        protected virtual void RemoveEditor(C1Feedback feedback)
        {
            if (feedback == null)
                return;

            if (editors.ContainsKey(feedback))
            {
                DestroyImmediate(editors[feedback]);
                editors.Remove(feedback);
            }
        }


        protected virtual void PlayFeedback(int id)
        {
            SerializedProperty property = c1feedbacks.GetArrayElementAtIndex(id);
            C1Feedback feedback = property.objectReferenceValue as C1Feedback;
            feedback.Play();
        }

    }
}
