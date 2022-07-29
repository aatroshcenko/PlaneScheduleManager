import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent } from './app.component';
import { DevicesComponent } from './components/devices/devices.component';
import { DeviceStatusPipe } from './pipes/device-status.pipe';

@NgModule({
  declarations: [
    AppComponent,
    DevicesComponent,
    DeviceStatusPipe
  ],
  imports: [
    BrowserModule, HttpClientModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
