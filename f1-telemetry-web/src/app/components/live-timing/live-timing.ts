import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Subscription } from 'rxjs';
import { SignalrService } from '../../core/services/signalr';
import { LapTime } from '../../core/models/lap-time';
import { LapTimeFormatPipe } from '../../core/pipes/lap-time-format-pipe';

@Component({
  selector: 'app-live-timing',
  standalone: true,
  imports: [CommonModule, LapTimeFormatPipe],
  templateUrl: './live-timing.html',
  styleUrl: './live-timing.scss'
})
export class LiveTimingComponent implements OnInit, OnDestroy {
  laps: LapTime[] = [];
  private subscription!: Subscription;

  constructor(
    private signalrService: SignalrService,
    private cdr: ChangeDetectorRef // <-- Injetamos o ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.subscription = this.signalrService.lapTime$.subscribe(lap => {
      console.log('🏁 Nova volta recebida para a tabela:', lap);
      
      // Coloca a nova volta no topo
      this.laps.unshift(lap);
      if (this.laps.length > 50) this.laps.pop();

      // Força o Angular a atualizar o HTML IMEDIATAMENTE!
      this.cdr.detectChanges();
    });
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
