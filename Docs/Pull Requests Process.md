## Using Feature Branches and Pull Requests in GitHub

### **1. Create a New Feature Branch**  
Before making any changes, create a separate branch for your feature.  

```bash
# Ensure you're on the master branch
git checkout master  

# Pull the latest changes
git pull origin master  

# Create and switch to a new feature branch
git checkout -b feature-branch-name
```

---

### **2. Make Changes & Commit**  
Edit your files and commit the changes.  

```bash
# Check modified files
git status  

# Add changes to staging
git add .  

# Commit the changes with a meaningful message
git commit -m "Added new feature: XYZ"
```

---

### **3. Push the Feature Branch to GitHub**  
Upload your branch to GitHub.  

```bash
git push origin feature-branch-name
```

---

### **4. Create a Pull Request (PR)**  
1. Go to your repository on GitHub.  
2. Click on **"Pull Requests"**.  
3. Click **"New Pull Request"**.  
4. Select **base branch** (e.g., `master`) and **compare branch** (your feature branch).  
5. Add a title and description explaining the changes.  
6. Click **"Create Pull Request"**.  

---

### **5. Review & Merge the PR**  
- Your team can **review** and provide feedback.  
- You can make additional commits if needed.  
- Once approved, click **"Merge Pull Request"**.  
- Delete the feature branch after merging to keep the repo clean.  

```bash
# Delete the local branch
git branch -d feature-branch-name  

# Delete the remote branch
git push origin --delete feature-branch-name
```

---

### **6. Update Your Local Master Branch**  
```bash
git checkout master  
git pull origin master  
```

Now you're ready to start another feature! 🚀
