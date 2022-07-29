import { Pipe, PipeTransform } from '@angular/core';
import { DeviceStatus } from '../models/device-status';

@Pipe({
  name: 'deviceStatus'
})
export class DeviceStatusPipe implements PipeTransform {

  transform(value: DeviceStatus): string {
    return DeviceStatus[value];
  }

}
