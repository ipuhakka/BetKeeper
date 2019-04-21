
export function betResultToRadioButtonValue(betResult){
  switch(betResult){
    case false:
      return 0;
    case true:
      return 1;
    default:
      return -1;
  }
}
