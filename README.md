# 🧠 Ollama Prompt Pipeline: Enriching LLM Calls with Ease

This project showcases how to use a **pipeline architecture** to enrich prompts for a large language model (LLM) such as Ollama, making it extremely simple to create structured, context-aware outputs (e.g., HTML emails) based on common activities like generating event invitations.

---

## ✨ What It Does

The system allows you to define **templates** with associated **prompt enrichers** that manipulate and enrich the prompt before it’s sent to the LLM engine. The LLM response is then returned in a structured format, such as JSON or HTML.

### Example Use Case: Event Invitation Email

Given a `ConversationRequest` with a `Template = "eventInvitation"`, the pipeline:

1. Loads the configured enrichers:
   ```json
   {
     "Name": "eventInvitation",
     "Enrichers": [
       "PromptNormalizer",
       "ScopeExtractorEnricher",
       "UserEnricher",
       "EventMetadataEnricher"
     ],
     "OutputFormat": "html",
     "Cardinality": "individual",
     "Actions": [ "persist", "Email" ]
   }
   ```
2. Sequentially applies each enricher to the prompt.
3. Sends the enriched prompt to the LLM engine.
4. Returns a structured response, e.g.:
   ```json
   {
     "status": "Generated",
     "result": "<html>...</html>"
   }
   ```

---
## ⚙️ How It Works

The core of the architecture is centered around a pipeline that receives a conversation request, enriches the prompt, and generates a structured response using the selected LLM engine.

Here’s a breakdown of the steps involved in the `Generate` endpoint:

1. **Receive the Conversation Request**  
   The API receives a `ConversationRequest` that includes the selected `Template` and `Engine`, along with any other input data.
   ```csharp
   public async Task<IActionResult> Generate([FromBody] ConversationRequest request, CancellationToken token)
   ```

2. **Build the Template Pipeline**  
   Based on the provided `Template` name, the system uses `pipelineEnricherBuilder` to load and build the full template definition, including all configured enrichers, output format, and actions.
   ```csharp
   Template template = await pipelineEnricherBuilder.BuildAsync(request.Template);
   ```
   🔍 This makes the system flexible: you can change behavior just by changing configuration.

3. **Create the LLM Engine**  
   Depending on the engine name in the request (e.g., `ollama`), the appropriate implementation is instantiated using `_engineFactory`.
   ```csharp
   var engine = await _engineFactory.CreateAsync(request.Engine);
   ```
   🔁 This allows supporting multiple LLM engines in a plug-and-play way.

4. **Run Prompt Enrichers (If Any)**  
   If the template defines any enrichers, they are executed sequentially. Each enricher enriches the prompt context with specific domain or user data.
   ```csharp
   if (template.Enrichers.Any()) 
   {
       await engine.EnrichAsync(request, template.Enrichers, token);
   }
   ```
   🧠 Enrichers are composable and follow the `IPromptEnricher` interface to add specific content like user info, event metadata, normalization, etc.

5. **Generate Final Output with the Engine**  
   After each enrichment, the prompt is sent to the selected engine to generate the final output.
   ```csharp
   var response = await engine.GenerateAsync(request, token);
   ```
   📤 This is where the prompt reaches the LLM with all required context.

6. **Return Structured Result**  
   The final result is returned to the caller, wrapped in a JSON structure for consistency.
   ```csharp
   return Ok(new { status = "Generated", result = response });
   ```
   📦 Useful for clients or frontends expecting structured data (e.g., JSON, HTML).

---

This flow abstracts the complexity of working with LLMs by allowing templates and enrichers to drive how a prompt is composed and handled, making it both extensible and maintainable.


Each `Enricher` implements:

```csharp
public interface IPromptEnricher
{
    Task EnrichAsync(PromptContext context);
}
```

This makes each enricher modular and composable.

---

## 🧱 Prompt Enrichers: Modular Intelligence in Your Pipeline

Each component in the enrichment pipeline implements the `IPromptEnricher` interface:

```csharp
public interface IPromptEnricher
{
    Task EnrichAsync(PromptContext context);
}
```

This interface powers the modular design of the pipeline. Each enricher is responsible for **augmenting the prompt context**—effectively preparing the input that will be sent to the LLM—and optionally updating the **system prompt**, influencing downstream enrichers or the final generation request.

### 🔁 How the Enrichment Pipeline Works

When a template defines a list of enrichers like this:

```json
"Enrichers": [
  "PromptNormalizer",
  "ScopeExtractorEnricher",
  "UserEnricher",
  "EventMetadataEnricher"
]
```

The engine will process each enricher **sequentially**, and for **each enricher**:

1. **Prepare a new `ConversationRequest`**  
   The enricher starts by deciding what it needs from the model (e.g., summarization, data extraction, transformation).
   
2. **Build a prompt tailored for that LLM call**  
   This might include previous messages, a system prompt, or just a clean slate depending on the enrichment logic.

3. **Specify the response format**  
   Enrichers can define whether the expected result is plain text, structured JSON, or another format like markdown or HTML.

4. **Decide if previous context is relevant**  
   For example, some enrichers like `PromptNormalizer` may not need previous conversation history, while `UserEnricher` might build on earlier context.

5. **Make an LLM call via the engine**  
   The `EnrichAsync()` method uses the engine to send its prompt and receive a response.

6. **Update the `PromptContext`**  
   After receiving the response, the enricher:
   - Updates the `PromptContext` with relevant values.
   - Optionally adds information to the **system prompt** (so future enrichers or the final generation are aware of it).
   - May store structured data for use in final output generation or action triggers (e.g., email sending, persistence, etc.).

---

### 🧩 Examples of Enrichers

#### ✅ `PromptNormalizer`
- **Goal:** Sanitize or normalize input from the user.
- **Prompt:** Reformulate the user’s original input into a more LLM-friendly version.
- **Output:** A cleaned or expanded prompt for subsequent enrichers.

#### 🕵️ `ScopeExtractorEnricher`
- **Goal:** Extract scope or context from the user's request.
- **Prompt:** Ask the model to identify what the conversation is about.
- **Output:** Extracted keywords or tags that may influence the system prompt.

#### 👤 `UserEnricher`
- **Goal:** Add personalized context based on the current user (e.g., preferences, prior interactions).
- **Prompt:** Generate a user profile summary or inject historical interactions.
- **Output:** Influences the system prompt with tailored information.

#### 📅 `EventMetadataEnricher`
- **Goal:** Inject data about a specific event (e.g., time, location, title).
- **Prompt:** Either compose a description or validate event data.
- **Output:** Fills out variables for the final generation phase (e.g., email invitation content).

---

### 🔄 Chaining Enrichers

Each enricher passes along its updated `PromptContext`, meaning later enrichers can build upon earlier ones.

This makes it easy to:
- Compose rich, multi-step prompt pipelines.
- Add new enrichers without changing previous logic.
- Ensure clean separation of concerns.

---

### 🧠 Influence via System Prompt

Enrichers can optionally inject data into the **system prompt** (the part of the message history that sets the tone or rules for the LLM).

For example:
> "The user is attending an event called *Developer Days 2025*. Please personalize your output accordingly."

This enables powerful orchestration of LLM behavior **without changing the final user prompt.**

---

### ⚠️ Important Considerations

- Be mindful of **latency**: each enricher makes a round-trip to the model.
- Enrichers should be **idempotent**: they must handle retries or failures gracefully.
- Logging intermediate context is helpful for **debugging and observability**.

---

This approach turns prompt engineering into **prompt architecture**, where each part of your logic becomes reusable, testable, and independently maintainable.


## ✅ Advantages of This Architecture

- **Modularity:** Add or remove enrichers without touching the core logic.
- **Reusability:** Enrichers can be shared across templates and use cases.
- **Configurability:** Templates are JSON-configurable, making it easy to define new behaviors.
- **Extensibility:** Easy to add actions like `persist`, `email`, etc.
- **Structure:** Prompts and responses are structured to fit HTML, JSON, or other formats.

---

## ⚠️ Trade-Offs and Challenges

### 🐢 Performance

The flexibility comes at a cost:
- **Sequential enrichment** can slow down response time, especially as the number or complexity of enrichers grows.
- Enrichers like `UserEnricher` or `EventMetadataEnricher` may rely on I/O operations (e.g., fetching metadata), introducing additional latency.

### ⚒️ Debuggability

- Tracing the final prompt can be tricky due to multiple transformation steps.
- Each enricher should ideally log its before/after states to aid debugging.

---

## 🏗️ Future Improvements

- **Parallelizable enrichment** where dependencies allow.
- **Prompt caching** to avoid repeating enrichment for similar requests.
- **Prompt visualizer** to inspect prompt transformations step-by-step.

---

## 🧪 Example Templates

You can define multiple templates like:

```json
{
  "Name": "followUpEmail",
  "Enrichers": ["PromptNormalizer", "UserEnricher"],
  "OutputFormat": "markdown",
  "Cardinality": "bulk",
  "Actions": ["persist"]
}
```

Just plug it into a request and the system will take care of everything.

---

## 🚀 Getting Started

1. Clone the repo
2. Run the API
3. POST to `/generate` with a payload like:

```json
{
  "Template": "eventInvitation",
  "Engine": "ollama",
  "Input": {
    "event": "Team Meetup",
    "date": "2025-04-20",
    "user": "Alice"
  }
}
```

---

## 👋 Why It Matters

This pattern is extremely useful for real-world LLM applications where context, formatting, and dynamic data matter. It demonstrates how **prompt engineering** can evolve into a **structured, pipeline-driven engineering discipline**.

---


## 🧪 Running Locally with Ollama

To make this pipeline work, you need to run an LLM engine locally — in this case, [**Ollama**](https://ollama.com/), which exposes models like `llama2`, `mistral`, or `gemma` through a local HTTP API.

---

### 🧰 Step 1: Install Ollama

Ollama runs locally as a **background service (daemon)** on your machine. Install it using one of the following:

#### 💻 macOS (with Homebrew)
```bash
brew install ollama
```

#### 🐧 Linux
```bash
curl -fsSL https://ollama.com/install.sh | sh
```

#### 🪟 Windows
1. Download the installer from: https://ollama.com/download
2. Run the installer and ensure Ollama starts automatically.

After installation, start the Ollama server:

```bash
ollama run llama2
```

> 💡 You can replace `llama2` with other supported models like `mistral`, `gemma`, or custom fine-tuned models.

---

### 🧠 How Ollama Works (Client–Server Model)

Ollama runs as a **local server** (typically on `http://localhost:11434`) and exposes a **REST API** that you can send JSON requests to. It handles:
- Model loading
- Prompt input
- Streaming or final output generation

Your app (this pipeline project) is a **client** that connects to the Ollama API using an HTTP client. This separation means:
- You can swap models by changing the request.
- You can keep Ollama running in the background and re-use it across sessions.
- You could even point to a remote Ollama instance if needed.

---

### ⚙️ Step 2: Make Sure Ollama is Running

You need to have the model loaded and running before starting your app. For example:

```bash
ollama run llama2
```

This will:
- Start the Ollama HTTP server
- Load the `llama2` model into memory
- Keep the service running on `http://localhost:11434`

---
 

---

## 🧩 Recap

- Ollama must be installed and running (`ollama run <model>`)
- Your app talks to it via HTTP
- You define what model and prompt to use from your app layer
- Ollama takes care of the heavy lifting locally — no cloud needed

---

Let me know if you'd like to support **model download automation**, Docker-based setup, or remote Ollama integration as an enhancement!
