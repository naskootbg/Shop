var _change = {
  Sht: "Щ",
  Zh: "Ж",
  Ts: "Ц",
  Ch: "Ч",
  Sh: "Ш",
  Yu: "Ю",
  Ya: "Я",
  sht: "щ",
  ts: "ц",
  ch: "ч",
  sh: "ш",
  yu: "ю",
  ya: "я",
  zh: "ж",
  a: "а",
  b: "б",
  v: "в",
  g: "г",
  d: "д",
  e: "е",
  z: "з",
  i: "и",
  y: "й",
  k: "к",
  l: "л",
  m: "м",
  n: "н",
  o: "о",
  p: "п",
  r: "р",
  s: "с",
  t: "т",
  u: "у",
  f: "ф",
  h: "х",
  
  A: "А",
  B: "Б",
  V: "В",
  G: "Г",
  D: "Д",
  E: "Е",
  Z: "З",
  I: "И",
  Y: "Й",
  K: "К",
  L: "Л",
  M: "М",
  N: "Н",
  O: "О",
  P: "П",
  R: "Р",
  S: "С",
  T: "Т",
  U: "У",
  F: "Ф",
  H: "Х", }  
export function changeText(text) {
  if (text) {
    var last2 = text.slice(-2);
    if (last2 === "ia" || last2 === "ya") {
      text = text.replace(last2, "ия");
    }
    Object.keys(_change).forEach(toSplit => text = text.split(toSplit).join(_change[toSplit]))
  }
  
    return text;

}
