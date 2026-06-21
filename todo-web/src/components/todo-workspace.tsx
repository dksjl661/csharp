"use client";

import { FormEvent, useEffect, useMemo, useState } from "react";
import { Label, Todo, todoApi } from "@/lib/api";
import styles from "@/app/page.module.css";

type Filter = "all" | "active" | "done";

const filters: { id: Filter; label: string }[] = [
  { id: "all", label: "All tasks" },
  { id: "active", label: "In progress" },
  { id: "done", label: "Complete" },
];

export function TodoWorkspace() {
  const [todos, setTodos] = useState<Todo[]>([]);
  const [labels, setLabels] = useState<Label[]>([]);
  const [filter, setFilter] = useState<Filter>("all");
  const [title, setTitle] = useState("");
  const [labelId, setLabelId] = useState<string>("");
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    async function loadWorkspace() {
      try {
        const [nextTodos, nextLabels] = await Promise.all([todoApi.getTodos(), todoApi.getLabels()]);
        setTodos(nextTodos);
        setLabels(nextLabels);
      } catch (loadError) {
        setError(loadError instanceof Error ? loadError.message : "Unable to connect to the Todo API.");
      } finally {
        setIsLoading(false);
      }
    }

    void loadWorkspace();
  }, []);

  const visibleTodos = useMemo(() => {
    if (filter === "active") return todos.filter((todo) => !todo.isCompleted);
    if (filter === "done") return todos.filter((todo) => todo.isCompleted);
    return todos;
  }, [filter, todos]);

  const completedCount = todos.filter((todo) => todo.isCompleted).length;
  const completion = todos.length === 0 ? 0 : Math.round((completedCount / todos.length) * 100);

  async function createTodo(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!title.trim()) return;

    setIsSaving(true);
    setError(null);
    try {
      const todo = await todoApi.createTodo({
        title,
        isCompleted: false,
        labelId: labelId ? Number(labelId) : null,
      });
      setTodos((current) => [...current, todo]);
      setTitle("");
      setLabelId("");
    } catch (saveError) {
      setError(saveError instanceof Error ? saveError.message : "Unable to create this task.");
    } finally {
      setIsSaving(false);
    }
  }

  async function toggleTodo(todo: Todo) {
    setError(null);
    try {
      const updated = await todoApi.updateTodo(todo.id, {
        title: todo.title,
        description: todo.description ?? undefined,
        isCompleted: !todo.isCompleted,
        labelId: todo.label?.id ?? null,
      });
      setTodos((current) => current.map((item) => (item.id === updated.id ? updated : item)));
    } catch (updateError) {
      setError(updateError instanceof Error ? updateError.message : "Unable to update this task.");
    }
  }

  async function removeTodo(id: number) {
    setError(null);
    try {
      await todoApi.deleteTodo(id);
      setTodos((current) => current.filter((todo) => todo.id !== id));
    } catch (deleteError) {
      setError(deleteError instanceof Error ? deleteError.message : "Unable to delete this task.");
    }
  }

  return (
    <main className={styles.workspace}>
      <section className={styles.hero} aria-labelledby="page-title">
        <div>
          <p className={styles.eyebrow}>PERSONAL SYSTEM / WEEK 24</p>
          <h1 id="page-title">Make room for<br />what matters.</h1>
          <p className={styles.intro}>A focused view of your backend-backed work, organized with honest priorities.</p>
        </div>
        <div className={styles.progressCard} aria-label={`${completion}% of tasks completed`}>
          <span>Completion</span>
          <strong>{completion}%</strong>
          <div className={styles.progressTrack}><i style={{ width: `${completion}%` }} /></div>
          <small>{completedCount} of {todos.length} tasks complete</small>
        </div>
      </section>

      <section className={styles.board} aria-label="Todo workspace">
        <aside className={styles.sidebar}>
          <div className={styles.brand}>TASK / SET</div>
          <nav aria-label="Task filters">
            {filters.map((item) => (
              <button
                className={filter === item.id ? styles.filterActive : styles.filter}
                key={item.id}
                onClick={() => setFilter(item.id)}
                type="button"
              >
                {item.label}
                <span>{item.id === "all" ? todos.length : item.id === "done" ? completedCount : todos.length - completedCount}</span>
              </button>
            ))}
          </nav>
          <div className={styles.labelLegend}>
            <p>Labels</p>
            {labels.map((label) => (
              <div key={label.id}><i style={{ backgroundColor: label.color }} />{label.name}</div>
            ))}
          </div>
        </aside>

        <section className={styles.content}>
          <div className={styles.contentHeader}>
            <div><p className={styles.kicker}>FOCUS LIST</p><h2>{filters.find((item) => item.id === filter)?.label}</h2></div>
            <span className={styles.today}>{new Intl.DateTimeFormat("en", { weekday: "long", month: "short", day: "numeric" }).format(new Date())}</span>
          </div>

          <form className={styles.addForm} onSubmit={createTodo}>
            <label className={styles.titleField}>
              <span className="srOnly">Task title</span>
              <input value={title} onChange={(event) => setTitle(event.target.value)} placeholder="Capture a task…" maxLength={200} />
            </label>
            <label className={styles.labelSelect}>
              <span className="srOnly">Label</span>
              <select value={labelId} onChange={(event) => setLabelId(event.target.value)}>
                <option value="">No label</option>
                {labels.map((label) => <option key={label.id} value={label.id}>{label.name}</option>)}
              </select>
            </label>
            <button className={styles.addButton} disabled={isSaving} type="submit">{isSaving ? "Adding…" : "Add task"}</button>
          </form>

          {error && <p className={styles.error} role="alert">{error}</p>}
          {isLoading ? <p className={styles.status}>Loading your workspace…</p> : (
            <ul className={styles.todoList}>
              {visibleTodos.map((todo) => (
                <li className={todo.isCompleted ? styles.todoDone : styles.todo} key={todo.id}>
                  <button aria-label={`Mark ${todo.title} as ${todo.isCompleted ? "incomplete" : "complete"}`} className={styles.check} onClick={() => void toggleTodo(todo)} type="button">
                    {todo.isCompleted && "✓"}
                  </button>
                  <div className={styles.todoCopy}><strong>{todo.title}</strong>{todo.description && <span>{todo.description}</span>}</div>
                  {todo.label ? <span className={styles.labelPill} style={{ borderColor: todo.label.color, color: todo.label.color }}>{todo.label.name}</span> : <span className={styles.noLabel}>No label</span>}
                  <button aria-label={`Delete ${todo.title}`} className={styles.delete} onClick={() => void removeTodo(todo.id)} type="button">Remove</button>
                </li>
              ))}
              {visibleTodos.length === 0 && <li className={styles.empty}>No tasks in this view. Capture the next useful thing.</li>}
            </ul>
          )}
        </section>
      </section>
    </main>
  );
}
