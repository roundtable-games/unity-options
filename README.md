# 'Optional Values' utility for Unity
A small Unity package that provides a serializable 'Option' implementation.

The `Option<T>` implementation works much like option types in other languages. An `Option<T>` value either wraps some value or no value. To extract the wrapped value, the caller must handle the case that no value is provided.

Compared to C#'s built-in nullable types, this utility type 1) more explicitly requires the 'no value' case to be handled, and 2) can be serialized and used in the inspector to enable the design of optional data fields.

Designers can use a basic checkbox to specify whether the option has a value. If it does, the property drawer allows the value to be configured as a standard property. Otherwise, the value field is hidden. The property drawer draws simple 1-line properties inline, as the property would normally appear, and draws more complex properties (such as arrays) underneath the toggle-able option label.

<img width="557" height="217" alt="image" src="https://github.com/user-attachments/assets/24b231be-cbd1-44d3-a93d-447cd2fed478" />

# Shorthand syntax
In code, some shorthand syntax is provided to simplify working with `Option<T>` types:

`Option.None` evaluates to a simple struct type that can be implicitly converted to any `Option<T>.None` equivalent, such that the expression `Option<SomeComplexType> myValue = Option.None` has the same effective behavior as `Option<SomeComplexType> myValue = Option<SomeComplexType>.None`.

<img width="525" height="97" alt="image" src="https://github.com/user-attachments/assets/4115a5b3-cac5-43df-92c0-bc0bf340698c" />

`Option.Some(T)` can also be used as a substitute for `Option<T>.Some(T)`. However, a simpler implicit conversion also exists that allows `Option<T>` to be populated from literal values (i.e., `Option<int> value = 5` is equivalent to `Option<int> value = Option.Some(5)`).

<img width="555" height="125" alt="image" src="https://github.com/user-attachments/assets/02c890e8-9021-4e72-a8d8-5b71d7dd84b2" />

# Install via git URL
To use this package in your Unity project, install via git url using the following link:
`https://github.com/roundtable-games/unity-options.git`
