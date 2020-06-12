
// var dc = draw_get_color();
// draw_set_color(c_red);
// draw_rectangle(x-100,y-100,x+100,y+100);
// draw_text_transformed(x,y,"BURH",5,5,0);
// draw_set_color(dc);

if (isRabbit) pal_swap_set(pal_rabbits, 5, false);
else pal_swap_set(pal_mainchar, palette + 1, false);
draw_self();
pal_swap_reset();
