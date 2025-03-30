+++
showonlyimage = false
draft = false
image = "img/portfolio/cpp_template_error.png"
date = "2025-03-30"
title = "C++ TIL"
weight = 0
+++

Compiler Error C2908: Confusing error, simple problem.
<!--more-->

Terms:
# Template specialization: when a templated thing is declared with a specific set of template params (not generalized) 
# Explicit specialization: when an implementation is provided for a specific set of template parameters

Typically, in a translation unit, using something before its been declared is a problem. Simple solution (usually): move the declarationg before the thing using it. Usually the compiler output is something like "blah is undefined/no matching function", but when templates are involved, the terminology of the output is fairly confusing:

```
error C2908: explicit specialization; 'ItemT *Datum::Get<Datum*>(void)' has already been instantiated
error C2908:         with
error C2908:         [
error C2908:             ItemT=Datum *
error C2908:         ]
```

Here's the code causing the issue:
```
template<>
int32_t Datum::Get<int32_t>() { ... }

template<>
int32_t Datum::GetWithReferenceResolve<int32_t>()
{
    Datum * pCurrent = this;
    pCurrent = pCurrent->Get< Datum* >();
    return pCurrent->Get< int32_t >();
}

// LOTS of other code before this line...

template<>
Datum* Datum::Get<Datum*>()
```

The error is caused by `pCurrent->Get< Datum* >()`. The solution is to just order `Datum::Get<Datum*>()` prior to `GetWithReferenceResolve` of course. I spun my wheels on this a bit before checking the docs...

Docs:
https://learn.microsoft.com/en-us/cpp/error-messages/compiler-errors-2/compiler-error-c2908?view=msvc-170