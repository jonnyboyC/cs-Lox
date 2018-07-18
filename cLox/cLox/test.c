#include <stdio.h>

struct Node {
	int data;
	struct Node *next;
};

void push(struct Node** head_ref, int data) {
	struct Node* new_node = (struct Node*)malloc(sizeof(struct Node));

	new_node->data = data;
	new_node->next = (*head_ref);

	(*head_ref) = new_node;
}

void printList(struct Node *node, void(*fptr)(void *)) {
	while (node != NULL) {
		(*fptr)(node->data);
		node = node->next;
	}
}

void printInt(void *n) {
	printf(" %d", *(int *)n);
}

int main() {
	struct Node *start = NULL;

	int arr[] = { 1, 2, 3, 4, 5 };
	int i;
	for (i = 4; i >= 0; i--) {
		push(&start, &arr[i]);
	}

	printf("Created linked list");
	printList(start, printInt);
}