S0UR3
=====

C# MKB populator

I use this to avoid manualy adding files to my project MKB.

You need C# (I use 2010 express) to open and build the project.

The populator is young and has no error handling or safe anything. I use it as is but others should take care and backup thier projects or best would probably be to first run this on a test project.


User Manual:

Populate MKB
============

  Every .cpp and .h file in the folder of the MKB will be added to the files secotion of the MKB. Existing file entries will be overridden. It is probably a good idea to backup your MKB before using this for the firts time.
  
  This operation will also add any .png files in the data folder to group files named and located by subdirectory.

Step 1. Drag and drop the MKB into the window.
Step 2. Click populate MKB.

Add Class
=========

  The ClassName.h, ClassName.cpp templates will be used to create a new class with the given name in the directory of the MKB.
  
Step 1. Drag and drop the MKB into the window.
Step 2. Click + class.
