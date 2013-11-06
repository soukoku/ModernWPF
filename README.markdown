ModernWPF
==============================

Project Info
--------------------------------------
This is a small lib for creating modern-style
Windows application in WPF 4. This was formerly
the MetroWPF lib. Docs on using this lib
will come out soon.

Some Features
--------------------------------------
+ Easily create custom border-less window without using WindowStyle="None"; the chrome-less window will automatically support resizing via drop-shadow like VS 2013 or Office 2013.
+ All standard chrome behaviors are supported (caption area, icon area, aero snap, etc.).
+ Does not require inheriting from special Window class for developers who may have their own base Window classes.
+ Modern theme in this lib is optional. Developer is free to complete re-style the border-less windows as a blank canvas using other theming lib.
+ Can set different border color for different windows in a single process.