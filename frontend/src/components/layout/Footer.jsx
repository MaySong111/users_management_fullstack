export default function Footer() {
  const currentYear = new Date().getFullYear();

  return (
    <footer className="bg-white border-t border-gray-200 px-8 py-6 mt-auto">
      <div className="flex items-center justify-between flex-wrap gap-4">
        <div className="flex-1">
          <p className="text-sm text-gray-500">© {currentYear} Your Company. All rights reserved.</p>
        </div>
        
        <div className="flex items-center gap-3">
          <a href="#" className="text-sm text-gray-500 hover:text-purple-600 transition-colors">
            Privacy Policy
          </a>
          <span className="text-gray-300">•</span>
          <a href="#" className="text-sm text-gray-500 hover:text-purple-600 transition-colors">
            Terms of Service
          </a>
          <span className="text-gray-300">•</span>
          <a href="#" className="text-sm text-gray-500 hover:text-purple-600 transition-colors">
            Contact
          </a>
        </div>
      </div>
    </footer>
  );
}