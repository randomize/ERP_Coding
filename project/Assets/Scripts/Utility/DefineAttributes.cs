using System;

[AttributeUsage(AttributeTargets.All)]
public class DescriptionAttribute : Attribute
{
    private string m_Desc;
    public string description { get { return m_Desc; } }
	public DescriptionAttribute(string description = null)
    {
        m_Desc = description;
    }
}