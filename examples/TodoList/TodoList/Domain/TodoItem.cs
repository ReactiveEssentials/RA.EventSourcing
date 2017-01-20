﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using ReactiveArchitecture.EventSourcing;
using TodoList.Events;

namespace TodoList.Domain
{
    public class TodoItem : EventSourced
    {
        public TodoItem(Guid id, string description)
            : base(id)
        {
            if (description == null)
                throw new ArgumentNullException(nameof(description));

            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException(
                    $"{nameof(description)} cannot be empty.",
                    nameof(description));
            }

            RaiseEvent(new TodoItemCreated { Description = description });
        }

        private TodoItem(Guid id) : base(id)
        {
        }

        public static TodoItem Factory(
            Guid id,
            IEnumerable<IDomainEvent> pastEvents)
        {
            var todoItem = new TodoItem(id);
            todoItem.HandlePastEvents(pastEvents);
            return todoItem;
        }

        public string Description { get; private set; }

        public bool IsDeleted { get; private set; }

        public void Update(string description)
        {
            if (IsDeleted)
                throw new InvalidOperationException("This todo item is deleted. Deleted todo item cannot be updated.");

            if (description == null)
                throw new ArgumentNullException(nameof(description));

            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException(
                    $"{nameof(description)} cannot be empty.",
                    nameof(description));
            }

            RaiseEvent(new TodoItemUpdated { Description = description });
        }

        public void Delete()
        {
            if (IsDeleted)
            {
                Trace.WriteLine($"{nameof(Delete)} operation is performed for todo item already deleted.");
                return;
            }

            RaiseEvent(new TodoItemDeleted());
        }

        private void Handle(TodoItemCreated domainEvent)
        {
            Description = domainEvent.Description;
            IsDeleted = false;
        }

        private void Handle(TodoItemUpdated domainEvent)
        {
            Description = domainEvent.Description;
        }

        private void Handle(TodoItemDeleted domainEvent)
        {
            IsDeleted = true;
        }
    }
}
