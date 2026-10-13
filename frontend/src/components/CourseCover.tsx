import { useState } from 'react'

interface CourseCoverProps {
  imageUrl: string | null | undefined
  title: string
  size?: 'card' | 'hero' | 'thumb'
}

const palettes = [
  ['#1D2747', '#FFE45C'],
  ['#4B3BD6', '#F6F7FB'],
  ['#0F6E63', '#FFE45C'],
  ['#B5462E', '#FFF4D6'],
  ['#2B5FA8', '#FFE45C'],
]

function pickPalette(seed: string) {
  let hash = 0
  for (const char of seed) hash = (hash * 31 + char.charCodeAt(0)) >>> 0
  return palettes[hash % palettes.length]
}

export function CourseCover({ imageUrl, title, size = 'card' }: CourseCoverProps) {
  const [failed, setFailed] = useState(false)

  if (imageUrl && !failed) {
    return (
      <img
        className={`cover cover--${size}`}
        src={imageUrl}
        alt=""
        loading="lazy"
        onError={() => setFailed(true)}
      />
    )
  }

  const [background, accent] = pickPalette(title)

  return (
    <div className={`cover cover--${size} cover--generated`} style={{ background, color: accent }} aria-hidden="true">
      <span className="cover__initial">{title.trim().charAt(0).toLocaleUpperCase('tr-TR')}</span>
    </div>
  )
}
