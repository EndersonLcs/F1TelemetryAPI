import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'lapTimeFormat',
  standalone: true
})
export class LapTimeFormatPipe implements PipeTransform {

  transform(value: number | undefined): string {
    if (!value) return '--:--.---';

    const minutes = Math.floor(value / 60000);
    const seconds = Math.floor((value % 60000) / 1000);
    const milliseconds = value % 1000;

    // Formata os segundos com 2 dígitos e os milissegundos com 3 dígitos
    const formattedSeconds = seconds.toString().padStart(2, '0');
    const formattedMs = milliseconds.toString().padStart(3, '0');

    // Se a volta for menor que 1 minuto, não mostra os minutos
    if (minutes === 0) {
      return `${formattedSeconds}.${formattedMs}`;
    }

    return `${minutes}:${formattedSeconds}.${formattedMs}`;
  }
}
