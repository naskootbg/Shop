// src/utils/slugify.js
const cyrillicToLatinMap = {
  а: 'a', б: 'b', в: 'v', г: 'g', д: 'd', е: 'e', ж: 'zh', з: 'z',
  и: 'i', й: 'y', к: 'k', л: 'l', м: 'm', н: 'n', о: 'o', п: 'p',
  р: 'r', с: 's', т: 't', у: 'u', ф: 'f', х: 'h', ц: 'ts', ч: 'ch',
  ш: 'sh', щ: 'sht', ъ: 'a', ь: '', ю: 'yu', я: 'ya'
}

export function slugify(text) {
  return text
    .toLowerCase()
    .split('')
    .map(c =>
      cyrillicToLatinMap[c] !== undefined
        ? cyrillicToLatinMap[c]
        : c.match(/[a-z0-9]/) ? c : '-'
    )
    .join('')
    .replace(/-+/g, '-')       // remove multiple dashes
    .replace(/^-|-$/g, '')     // trim dashes
}
