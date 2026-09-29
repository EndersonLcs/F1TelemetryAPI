import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SignalrService } from './core/services/signalr';
import { LiveTimingComponent } from './components/live-timing/live-timing';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [LiveTimingComponent], // Registra o componente para uso
  template: `
    <main style="background-color: #111; color: white; min-height: 100vh; padding: 20px; font-family: monospace;">
      <h1>🏁 F1 Live Telemetry - Pit Wall</h1>
      
      <app-live-timing></app-live-timing>
    </main>
  `
})

export class AppComponent implements OnInit {
  
  constructor(private signalrService: SignalrService) {}

  ngOnInit(): void {
    // Apenas liga o rádio. A tabela cuida do resto!
    this.signalrService.startConnection();
  }
}
