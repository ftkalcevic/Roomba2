import { NgModule }      from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpModule, JsonpModule } from '@angular/http';
import { FormsModule } from '@angular/forms';

import { AppComponent }  from './app.component';
import { RoombaMapComponent } from './roomba-map.component';

@NgModule({
    imports: [
        BrowserModule,
        FormsModule,
        HttpModule,
        JsonpModule],
    declarations: [
        AppComponent,
        RoombaMapComponent ],
  bootstrap:    [ AppComponent ]
})
export class AppModule { }
