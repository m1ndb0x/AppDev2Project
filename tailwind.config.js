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
          50: '#fff5f5',
          100: '#ffe4e4',
          200: '#ffc1c1',
          300: '#ff9b9b',
          400: '#ff7f50',  // Main coral color
          500: '#ff6b6b',
          600: '#ff5252',
          700: '#ff3838',
          800: '#ff1f1f',
          900: '#ff0505',
        },
        'custom-pink': '#FFB6C1',
        'custom-salmon': '#F88379',
      },
    },
  },
  plugins: [],
}