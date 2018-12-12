import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'numberToImg'
})
export class NumberToImgPipe implements PipeTransform {

  transform(value: number, icon: string= 'filter_' ): any {
    if ( value == null) {
      return '';
    }

    const val = value.toString(10);
    let ret = '';
    for (let i = 0; i < val.length; i++) {
        const nr = val[i];
        if (nr === '0' && icon === 'filter_' ) {
          ret += `<i class="material-icons">exposure_zero</i>`;
        } else {
        ret += `<i class="material-icons">filter_${val[i]}</i>`;
        }
    }
    // console.log(ret);
    return ret;
  }

}
