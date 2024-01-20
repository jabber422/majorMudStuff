This is a GDI+ drawing panel.  If you feed it a constant Queue\<TermCmd\> this will 
convert the queue into a 2D "grid" and store everything in a circular buffer.
The buffer stores the current state of the viewable data and gets drawn to the panel via GDI commands.