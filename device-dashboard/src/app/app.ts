import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { Device } from './device.model';
import { CommonModule } from '@angular/common';  
import { HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, HttpClientModule], 
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {

  devices: Device[] = [];
  private socket!: WebSocket;

  constructor(private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.socket = new WebSocket("ws://localhost:8080/ws");
    
    this.socket.onmessage = (event) => {
      const rawDevices = JSON.parse(event.data);
      console.log("RAW:", rawDevices);

      this.devices = rawDevices.map((d: any) => ({
        deviceId: d.DeviceId ?? d.deviceId,
        status: d.Status ?? d.status,
        isOnline: d.IsOnline ?? d.isOnline,
        lastHeartbeat: d.LastHeartBeat ?? d.LastHeartbeat ?? d.lastHeartbeat
          ? new Date(d.LastHeartBeat ?? d.LastHeartbeat ?? d.lastHeartbeat)
          : null
      }));
    
      console.log("MAPPED:", this.devices);
    
      this.cdr.detectChanges();
    };

  }
}
