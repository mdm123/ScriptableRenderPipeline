<div style="border: solid 1px #999; border-radius:12px; background-color:#EEE; padding: 8px; padding-left:14px; color: #555; font-size:14px;"><b>Draft:</b> The content on this page is complete, but it has not been reviewed yet.</div>

# Visual Effect component API

To create an instance of a [Visual Effect Graph Asset](VisualEffectGraphAsset.md) in a Scene, Unity uses the [Visual Effect component](VisualEffectComponent.md). The Visual Effect component attaches to GameObjects in your Scene and references a Visual Effect Graph Asset that defines the visual effect. This allows you to create different instances of effects at various positions and orientations, and control each effect independently. To control an effect at runtime, Unity provides C# API that you can use to modify the component and set [Property](Properties.md) overrides. 

This document presents common use cases and describes good practices to consider when you use the [component API](https://docs.unity3d.com/Documentation/ScriptReference/VFX.VisualEffect.html).

## Setting a Visual Effect Graph

To change the [Visual Effect Graph Asset](VisualEffectGraphAsset.md) at runtime, you can use the `effect.visualEffectAsset ` property. When you change the Visual Effect Graph Asset, this resets the value of certain properties on the component.

The values that reset are:

* **Total Time**: When you change the graph, the API calls the `Reset()` function which sets this value to 0.0f.
* **Event Attributes**: The component discards all Event [Attribues](Attributes.md).

The values that do **not** reset are:

* **Exposed Property Overrides**: If the new Visual Effect Graph Asset exposes a property that has the same name and type as a property from the previous Asset, the value for this property does not reset.
* **Random Seed** and **Reset Seed On Play Value**.
* **Default Event Override**.
* **Rendering Settings overrides**.

## Controlling play state

You can use the API to control effect playback.

### Common controls

* **Play** : `effect.Play()` or `effect.Play(eventAttribute)` if needing Event Attributes.
* **Stop** : `effect.Stop()` or `effect.Stop(eventAttribute)` if needing Event Attributes.
* **Pause** : `effect.pause = true` or  `effect.pause = false`. Unity does not serialize this change.
* **Step** : `effect.AdvanceOneFrame()`. This only works if `effect.pause` is set to `true`.
* **Reset Effect** : `effect.Reinit()` this also :
  * Resets `TotalTime` to 0.0f.
  * Re-sends the **Default Event** to the Visual Effect Graph Asset.
* **Play Rate** : `effect.playRate = value`. Unity does not serialize this change.

### Default Event

When the Visual Effect component (or the GameObject it attaches to) enables, it sends an [Event](Events.md) to the graph. By default, this Event is `OnPlay` which is the standard start for [Spawn Contexts](Contexts.md#spawn).

You can change the default Event in the following ways:

* On the [Visual Effect Inspector](VisualEffectComponent.md), change the **Initial Event Name** field.
* In the component API : `initialEventName = "MyEventName";`.
* In the component API : `initialEventID = Shader.PropertyToID("MyEventName";`.
* Using the [ExposedProperty Helper Class](ExposedPropertyHelper.md).

## Random Seed Control

Every effect instance has settings and controls for its random seed. You can modify the seed to influence the random values the Visual Effect Graph Asset uses.

* `resetSeedOnPlay = true/false`: Controls whether Unity computes a new random seed every time you call the `Play()` function. This causes each random value the Visual Effect Graph Asset uses to be different to what it was in previous simulations.
* `startSeed = intSeed`: Sets a manual seed that the **Random Number** Operator uses to create random values for this Visual Effect. Unity ignores this value if `resetSeedOnPlay` is set to `true`.

## Property Interface

To access the state and values of Exposed Properties, you can use multiple methods in the [Visual Effect component](VisualEffectComponent.md). Most of the API methods allow access to the property via the following methods:

* A `string` property name. This is easy to use, but is the least optimized method.
* An `int` property ID. To generate this ID from a string property name, you can use `Shader.PropertyToID(string name)`. This is the most optimized method.
* The [ExposedProperty Helper Class](ExposedPropertyHelper.md). This combines the ease of use the string property name provides with the efficiency of the integer property ID.

#### Checking for exposed properties

You can check if the component's Visual Effect Graph Asset contains a specific exposed property. To do this, you can use the method from the following group that corresponds to the property's type:

* `HasInt(property)`
* `HasUInt(property)`
* `HasBool(property)`
* `HasFloat(property)`
* `HasVector2(property)`
* `HasVector3(property)`
* `HasVector4(property)`
* `HasGradient(property)`
* `HasAnimationCurve(property)`
* `HasMesh(property)`
* `HasTexture(property)`
* `HasMatrix4x4(property)`

For each method, if the Visual Effect Graph Asset contains an exposed property of the correct type with the same name or ID that you pass in, the method returns `true`. Otherwise the methods returns `false`.

#### Getting the values of exposed properties

The component API allows you to get the value of an exposed property in the component's Visual Effect Graph Asset. To do this, you can use the method from the following group that corresponds to the property's type:

* `GetInt(property)`
* `GetUInt(property)`
* `GetBool(property)`
* `GetFloat(property)`
* `GetVector2(property)`
* `GetVector3(property)`
* `GetVector4(property)`
* `GetGradient(property)`
* `GetAnimationCurve(property)`
* `GetMesh(property)`
* `GetTexture(property)`
* `GetMatrix4x4(property)`

For each method, if the Visual Effect Graph Asset contains an exposed property of the correct type with the same name or ID that you pass in, the method returns the property's value. Otherwise the methods return the default value for the property type.

#### Setting the values of exposed properties

The component API allows you to set the value of an exposed property in the component's Visual Effect Graph Asset. To do this, you can use the method from the following group that corresponds to the property's type:

* `SetInt(property,value)`
* `SetUInt(property,value)`
* `SetBool(property,value)`
* `SetFloat(property,value)`
* `SetVector2(property,value)`
* `SetVector3(property,value)`
* `SetVector4(property,value)`
* `SetGradient(property,value)`
* `SetAnimationCurve(property,value)`
* `SetMesh(property,value)`
* `SetTexture(property,value)`
* `SetMatrix4x4(property,value)`

Each method overrides the value of the corresponding property with the value that you pass in.

#### Resetting property overrides and default values

The component API allows you to resetting property overrides back to their original values. To do this, use the `ResetOverride(property)` method.

## Events

### Sending Events

You can send [Events](Events.md) to the Visual Effect instance using the following API:

* `SendEvent(eventNameOrId)`
* `SendEvent(eventNameOrId, eventAttribute)`

The parameter `eventNameOrId` can be of the following types:

- a `string` event Name : easy to use but less optimized.
- an `int` event ID that can be generated and cached using `Shader.PropertyToID(string name)`
- an [ExposedProperty Helper Class](ExposedPropertyHelper.md) that will cache the `int` value corresponding to the string name

The optional EventAttribute parameter attaches an **Event Attribute Payload** to the event, so it can be processed by the Graph.

> Events are sent to the API then Consumed in the next Visual Effect Component Update, happening the next frame.

### Event Attributes

Event Attributes are [Attributes](Attributes.md) attached to [Events](Events.md) and that can be processed by the graph. Event Attributes are stored in a `VFXEventAttribute` class, created from an instance of a [Visual Effect](VisualEffectComponent.md), based on its currently set  [Visual Effect Graph Asset](VisualEffectGraphAsset.md).

#### Creating Event Attributes

In order to Create and Use a `VFXEventAttribute` use the `CreateVFXEventAttribute()` method of the `VisualEffect` component. If you plan on sending multiple times events using attributes, you will preferably cache this object so you can reuse it.

#### Setting Attribute Payload

Once Created, you can access an API similar to Has/Get/Set Properties in order to set the Attribute Payload.

* Has : `HasBool`, `HasVector3`, `HasFloat`,... To check if attribute is present
* Get : `GetBool`, `GetVector3`, `GetFloat`,... To get attribute value
* Set: `SetBool`, `SetVector3`, `SetFloat`,... To get attribute value

The full API Reference is available on [Scripting API Documentation](https://docs.unity3d.com/2019.3/Documentation/ScriptReference/VFX.VFXEventAttribute.html).

The attribute name or ID can be of the following types:

- a `string` attribute Name : easy to use but less optimized.
- an `int` attribute ID that can be generated and cached using `Shader.PropertyToID(string name)`
- an [ExposedProperty Helper Class](ExposedPropertyHelper.md) that will cache the int value corresponding to the string name

#### Life Cycle and Compatibility

Event Attributes, when created, are compatible with the Visual Effect Graph Asset that is currently set on the Visual Effect component. This means that you will be able to use the same `VFXEventAttribute` to send events to instances of the graph, as long as you do not change the `visualEffectAsset` property of the component to another Graph.

If you manage multiple Visual Effect instances in scene and want to share event payloads, you can cache one VFXEventAttribute and use it on all the instances.

#### Example (in a MonoBehaviour)

```c#
VisualEffect visualEffect;
VFXEventAttribute eventAttribute;

static readonly ExposedProperty positionAttribute = "Position"
static readonly ExposedProperty enteredTriggerEvent = "EnteredTrigger"

void Start()
{
	visualEffect = GetComponent<VisualEffect>();   
	// Caches an Event Attribute matching the
	// visualEffect.visualEffectAsset graph.
	eventAttribute = visualEffect.CreateVFXEventAttribute();
}

void OnTriggerEnter()
{
    // Sets some Attributes
    eventAttribute.SetVector3(positionAttribute, player.transform.position);
    // Sends the Event
    visualEffect.SendEvent(enteredTriggerEvent, eventAttribute);
}
```

## Debug Functionality

Some debug Functionality values can be get on every component:

* `aliveParticleCount` : return a read-back value of the alive particles in the whole effect. Readback of this value happens asynchronously every second, and it does return the value of a previous frame.
* `culled` return whether the effect was culled from any camera at the previous frame.