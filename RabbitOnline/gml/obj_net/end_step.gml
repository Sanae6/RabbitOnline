if (instance_number(obj_player) > 0 && room != rm_mainmenu){
    x = obj_player.x;
    y = obj_player.y;
    if (obj_player.sprite_index != sidx || (iidxtimer--<0 && obj_player.image_index != iidx) || pal != global.palSet || image_xscale != obj_player.image_xscale){
        sidx = obj_player.sprite_index;
        iidx = obj_player.image_index;
        if (iidxtimer < 0)iidxtimer = 5;
        pal = global.palSet;
        image_xscale = obj_player.image_xscale;
        wop_spritechange(sidx, obj_player.image_index, obj_player.image_speed, obj_player.image_xscale,global.palSet,obj_player.playerMode == 2 );
    }
    if (xprevious != x || yprevious != y) wop_movement(x,y);
}