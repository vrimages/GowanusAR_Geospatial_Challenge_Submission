using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Code created by Gaskellgames
/// </summary>

namespace Gaskellgames
{

    #region 'ReadOnly' Attribute

    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
        
    } // class end

    #endregion

    //----------------------------------------------------------------------------------------------------

    #region 'TagDropdown' Attribute

    [CustomPropertyDrawer(typeof(TagDropdownAttribute))]
    public class TagDropdownPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                EditorGUI.BeginProperty(position, label, property);

                var attrib = this.attribute as TagDropdownAttribute;

                if (attrib.UseDefaultTagFieldDrawer)
                {
                    property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
                }
                else
                {
                    // generate the taglist
                    List<string> tagList = new List<string>();
                    tagList.AddRange(UnityEditorInternal.InternalEditorUtility.tags);
                    string propertyString = property.stringValue;
                    int index = -1;
                    if (propertyString == "")
                    {
                        index = 0; // first index is a special case: Untagged
                    }
                    else
                    {
                        // check if entry matches and get the index
                        for (int i = 1; i < tagList.Count; i++)
                        {
                            if (tagList[i] == propertyString)
                            {
                                index = i;
                                break;
                            }
                        }
                    }

                    // draw the popup box with the current selected index
                    index = EditorGUI.Popup(position, label.text, index, tagList.ToArray());

                    // adjust the actual string value of the property based on the selection
                    if (index >= 1)
                    {
                        property.stringValue = tagList[index];
                    }
                    else
                    {
                        property.stringValue = "";
                    }
                }

                EditorGUI.EndProperty();
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }
        
    } // class end

    #endregion

    //----------------------------------------------------------------------------------------------------

    #region 'LineSeparator' Attribute

    [CustomPropertyDrawer(typeof(LineSeparatorAttribute))]
    public class LineSeparatorDraw : DecoratorDrawer
    {
        private readonly int _spacing = 10;
        
        public override void OnGUI(Rect position)
        {
            // ref to attribute
            LineSeparatorAttribute lineSeparatorAttribute = attribute as LineSeparatorAttribute;
            
            // define the line
            Rect separatorRect = new Rect(position.xMin, position.yMin + _spacing, position.width, lineSeparatorAttribute.Thickness);
            
            // draw line
            EditorGUI.DrawRect(separatorRect, new Color32(lineSeparatorAttribute.R, lineSeparatorAttribute.G, lineSeparatorAttribute.B, lineSeparatorAttribute.A));
        }

        public override float GetHeight()
        {
            LineSeparatorAttribute lineSeparatorAttribute = attribute as LineSeparatorAttribute;

            float totalSpacing = _spacing + lineSeparatorAttribute.Thickness + _spacing;

            return totalSpacing;
        }
    }
    
    #endregion
}
