+++
showonlyimage = false
draft = true
image = "img/portfolio/ISO_C++_Logo.svg"
date = "2021-10-21"
title = "C++ TIL"
weight = 0
+++

Static member definitions and ODR
<!--more-->

What does it mean if a variable is "odr-used"? What does ODR mean?

Consider the following code:

    class SomeClass
    {
    public:
        static const char* wellKnownKeyValue = "CommonString";
    };

... which won't compile. You can't define static members in-class, even if they're const, unless they're integral types:

    error C2864: 'SomeClass::wellKnownKeyValue': a static data member with an in-class initializer must have non-volatile const integral type or be specified as 'inline'

To resolve this, you simply declare the variable and define it outside the scope of the class. Great.

But what if you make the assignment a constexpr?

    class SomeClass
    {
    public:
        static constexpr const char* wellKnownKeyValue = "CommonString";
    };

Voila! The compiler is happy. But why?

Sources:
https://en.cppreference.com/w/cpp/language/static
https://en.cppreference.com/w/cpp/language/definition#ODR-use


