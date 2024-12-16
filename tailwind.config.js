/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./Views/**/*.{cshtml,html}",
    "./Views/**/*.cshtml",
    "./Pages/**/*.cshtml"
  ],
  theme: {
    extend: {
      colors: {
        coral: {
          light: '#FF8F70',
          DEFAULT: '#F88379',
          dark: '#FF6F30',
        },
        'custom-pink': '#FFB6C1',
        'custom-salmon': '#F88379',
      },
    },
  },
  plugins: [],
}