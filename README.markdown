ModernWPF
==============================

Project Info
--------------------------------------
This is a small lib for creating modern-style
Windows application in WPF 4. This was formerly
the MetroWPF lib. 

Some Features
--------------------------------------
+ Easily create border-less window with resizing drop-shadows like VS 2012 or Office 2013 without using WindowStyle="None" property.

+ All standard chrome behaviors are supported (caption area, icon area, aero snap, etc.).

+ Does NOT require inheriting from a custom Window class.

+ Modern theme in this lib is optional. The border-less windows is a blank canvas so developers can use other theming lib.

+ Chrome only applies to one window (unless set in styles) so different windows in a single process can have different border colors.
