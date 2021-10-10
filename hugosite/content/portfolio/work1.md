+++
showonlyimage = false
draft = false
image = "img/portfolio/ISO_C++_Logo.svg"
date = "2021-10-09"
title = "C++ TIL"
weight = 0
+++

What I learned in C++ today.
<!--more-->

It's better to use curly-braces for initialization. Consider:

    int i = 2.0f;
    int y = { 2.0f };

The first line will possibly emit a warning, depending on your compiler warning level. The second is treated as a compiler error and is apparently the superior way to assign values in C++.

Source: "A Tour of C++", Stroustrop.
