import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { SignalrService } from './services/signalr.service';
import { Observable, Subscription} from 'rxjs';
import { DeviceHeartbeat } from './models/device-heartbeat';
import { concatMap, map, scan, take, tap } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  readonly deviceHeartbeats$: Observable<DeviceHeartbeat[]>; 

  constructor(
    private readonly signalrService: SignalrService
  ) {
    this.deviceHeartbeats$ = signalrService.onReceiveDeviceHeartbeat()
      .pipe(
        scan((acc: Map<string, DeviceHeartbeat>, cv: DeviceHeartbeat)=> {
          acc.set(cv.deviceId, cv);
          return acc;
        }, new Map<string, DeviceHeartbeat>()),
        map((acc) => Array.from(acc.values()))
      );
  }

  ngOnInit(): void {
    this.signalrService.startConnection()
      .pipe(
        take(1)
      )
      .subscribe()
  }
}
