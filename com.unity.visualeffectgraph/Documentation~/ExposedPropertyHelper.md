<div style="border: solid 1px #999; border-radius:12px; background-color:#EEE; padding: 8px; padding-left:14px; color: #555; font-size:14px;"><b>Experimental:</b> This Feature is currently experimental and is subject to change in later major versions.</div>
# Exposed Property class

The `ExposedProperty` class is a helper class that caches a property ID based on the property's name. You can assign a the name of a Shader property as a string to the class and It automatically caches the integer value that `Shader.PropertyToID(string name)` returns. When you use this class in a Property, Event, or EventAttribute method in the [component API](ComponentAPI.md), it implicitly casts to this integer.

## Example usage

```C#
ExposedProperty m_MyProperty;
VisualEffect m_VFX;

void Start()
{
    m_VFX = GetComponent<VisualEffect>();
    m_MyProperty = "My Property"; // Assign A string
}

void Update()
{
    vfx.SetFloat(m_MyProperty, someValue); // Uses the int ID prototype
}
```

