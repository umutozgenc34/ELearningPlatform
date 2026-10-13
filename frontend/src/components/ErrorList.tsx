export function ErrorList({ messages }: { messages: string[] }) {
  if (messages.length === 0) return null

  return (
    <div className="error-box" role="alert">
      {messages.length === 1 ? (
        <p>{messages[0]}</p>
      ) : (
        <ul>
          {messages.map((message) => (
            <li key={message}>{message}</li>
          ))}
        </ul>
      )}
    </div>
  )
}
