import { Injectable, NgZone } from '@angular/core';
import { from, fromEvent, Observable, Subject, Subscriber } from 'rxjs';
import * as signalR from "@microsoft/signalr"
import { environment } from 'src/environments/environment';
import { DeviceHeartbeat } from '../models/device-heartbeat';
import { DeviceStatus } from '../models/device-status';

@Injectable({
  providedIn: 'root'
})
export class SignalrService {

  private readonly hubConnection: signalR.HubConnection;
  private readonly deviceHeartBeatSource: Subject<DeviceHeartbeat> = new Subject<DeviceHeartbeat>();

  constructor(
    private readonly zone: NgZone
  ) {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.hubUrl}?isManager=true&clientId=${environment.clientId}`, {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
      })
      .build();
    this.hubConnection.on('ReceiveDeviceHeartbeat', (heartbeat: DeviceHeartbeat) => {
      this.deviceHeartBeatSource.next(heartbeat);
    })
  }

  startConnection(): Observable<void> {
    return from(this.hubConnection.start())
  }

  onReceiveDeviceHeartbeat(): Observable<DeviceHeartbeat> {
    return this.getZonedObservable(
      this.deviceHeartBeatSource.asObservable()
    )
  }

  private getZonedObservable<T>(observable: Observable<T>): Observable<T> {
    return new Observable((observer: Subscriber<T>) => {
        const onNext = (value: T) => this.zone.run(() => observer.next(value));
        const onError = (error: any) => this.zone.run(() => observer.error(error));
        const onComplete = () => this.zone.run(() => observer.complete());
        return observable.subscribe(onNext, onError, onComplete);
    });
  }

  
}
