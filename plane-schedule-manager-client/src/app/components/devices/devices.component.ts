import { ChangeDetectionStrategy, Component, Input, OnInit } from '@angular/core';
import { DeviceHeartbeat } from 'src/app/models/device-heartbeat';

@Component({
  selector: 'app-devices',
  templateUrl: './devices.component.html',
  styleUrls: ['./devices.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DevicesComponent {
  @Input() heartbeats: DeviceHeartbeat[] = [];
}
