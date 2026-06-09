import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'bytesFormat'
})
export class BytesFormatPipe implements PipeTransform {

  transform(value: unknown, ...args: unknown[]): unknown {
    return null;
  }

}
