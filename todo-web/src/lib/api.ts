export type Label = {
  id: number;
  name: string;
  description: string | null;
  color: string;
};

export type Todo = {
  id: number;
  title: string;
  description: string | null;
  isCompleted: boolean;
  createdAt: string;
  label: Label | null;
};

export type TodoInput = {
  title: string;
  description?: string;
  isCompleted: boolean;
  labelId: number | null;
};

const apiUrl = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5261";

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  const response = await fetch(`${apiUrl}${path}`, {
    ...init,
    headers: { "Content-Type": "application/json", ...init?.headers },
  });

  if (!response.ok) {
    const body = await response.json().catch(() => null);
    throw new Error(body?.error ?? "The API request failed.");
  }

  return response.status === 204 ? (undefined as T) : response.json();
}

export const todoApi = {
  getTodos: () => request<Todo[]>("/api/todos"),
  getLabels: () => request<Label[]>("/api/labels"),
  createTodo: (input: TodoInput) =>
    request<Todo>("/api/todos", { method: "POST", body: JSON.stringify(input) }),
  updateTodo: (id: number, input: TodoInput) =>
    request<Todo>(`/api/todos/${id}`, { method: "PUT", body: JSON.stringify(input) }),
  deleteTodo: (id: number) => request<void>(`/api/todos/${id}`, { method: "DELETE" }),
};
