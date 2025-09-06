# 'Optional Values' utility for Unity
A small Unity package that provides a serializable 'Option' implementation.

The `Option<T>` implementation works much like option types in other languages. An `Option<T>` value either wraps some value or no value. To extract the wrapped value, the caller must handle the case that no value is provided.

In addition to the in-code utilities provided by the type, this `Option<T>` value can also be serialized and used in the inspector.

Designers can use a basic checkbox to specify whether the option has a value. If it does, the property drawer allows the value to be configured as a standard property. Otherwise, the value field is hidden. The property drawer draws simple 1-line properties inline, as the property would normally appear, and draws more complex properties (such as arrays) underneath the toggle-able option label.

<img width="557" height="217" alt="image" src="https://github.com/user-attachments/assets/24b231be-cbd1-44d3-a93d-447cd2fed478" />
