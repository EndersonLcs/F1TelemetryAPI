import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { LapTime } from '../models/lap-time';

@Injectable({
  providedIn: 'root'
})
export class SignalrService {
  private hubConnection: signalR.HubConnection | undefined;
  
  // O Subject é como um alto-falante. Quando chega uma volta, ele "grita" para a aplicação.
  private lapTimeSubject = new Subject<LapTime>();
  
  // Os componentes visuais (gráficos, tabelas) vão escutar esta variável ($ indica Observable)
  public lapTime$ = this.lapTimeSubject.asObservable();

  constructor() { }

  public startConnection = () => {
    // Ajuste a porta para a porta correta em que a API C# está rodando!
    this.hubConnection = new signalR.HubConnectionBuilder()
                            .withUrl('http://localhost:5209/hubs/telemetry') 
                            .build();

    this.hubConnection
      .start()
      .then(() => console.log('🟢 Conectado ao Pit Wall (SignalR)!'))
      .catch(err => console.error('🔴 Erro ao conectar com o SignalR: ', err));

    this.addReceiveLapListener();
  }

  private addReceiveLapListener = () => {
    if (this.hubConnection) {
      // Ouve exatamente o mesmo nome de evento que disparamos no C#
      this.hubConnection.on('ReceiveNewLap', (data: LapTime) => {
        this.lapTimeSubject.next(data); // Passa a volta recebida para o alto-falante do Angular
      });
    }
  }
}
